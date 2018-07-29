import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';
import { AutomationInfo, AutomationType } from '../models/AutomationInfo';
import { LightPanel } from '../models/LightPanel';
import { ClimatePanel } from '../models/ClimatePanel';

interface DashboardState {
    automations: AutomationInfo[];
    loading: boolean;
}

export class Home extends React.Component<RouteComponentProps<{}>, DashboardState> {
    constructor() {
        super();
        this.state = { automations: [], loading: true };

        this.refresh();
    }

    public refresh() {
        fetch('api/Automation')
            .then(response => response.json() as Promise<AutomationInfo[]>)
            .then(data => {
                this.setState({ automations: data, loading: false });
            });
    }

    public render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderAutomations(this.state.automations);

        return <div>{contents}</div>;
    }

    private renderAutomations(automations: AutomationInfo[]) {
        return <div className='row'>
            {automations.map(automation => {
                if (automation.type == AutomationType.Lights) {
                    return (<div className="col-md-3">
                        <LightPanel info={automation} />
                    </div>)
                }
                else if (automation.type == AutomationType.Climate) {
                    return (<div className="col-md-3">
                        <ClimatePanel info={automation} />
                    </div>)
                }
            }
            )}
        </div>;
    }
}
