import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';
import { AutomationInfo } from '../models/AutomationInfo';

interface Props {
    info: AutomationInfo
}

export class ClimatePanel extends React.Component<Props, AutomationInfo> {
    constructor(info: Props) {
        super();

        this.state = info.info;
    }

    public refresh(info: AutomationInfo) {
        this.state = info;
        return this.render();
    }

    public render() {
        let contents = this.renderAutomations(this.state);

        return <div>{contents}</div>;
    }

    public incrementCounter(automation: AutomationInfo) {
        fetch('api/Automation/Toggle(' + automation.id + ')', {
            method: 'POST'
        }).then(response => {
            //this.refresh();
        });
    }

    private renderAutomations(info: AutomationInfo) {
        return <div className={`automation-info ${info.enabledClass}`}>
            <h2>{info.title}</h2>
            <div>
                <img className="automation-icon" src={`images/${info.icon}`} />
                <span>{info.status}</span>
                <div>
                </div>
            </div>
        </div>;
    }
}