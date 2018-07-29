import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';
import { AutomationInfo } from '../models/AutomationInfo';

interface Props {
    info: AutomationInfo
}

export class LightPanel extends React.Component<Props, AutomationInfo> {
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

    public changeLight(e: React.MouseEvent<HTMLInputElement>) {
        fetch('api/Lights/Set(' + this.state.id + ')?turnOn=' + e.currentTarget.checked, {
            method: 'POST'
        })
            .then(response => response.json() as Promise<AutomationInfo>)
            .then(response => {
                this.setState(response);
            });
    }

    private renderAutomations(info: AutomationInfo) {
        return <div className={`automation-info ${info.enabledClass}`}>
            <h2>{info.title}</h2>
            <div>
                <img className="automation-icon" src={`images/${info.icon}`} />
                <span>{info.status}</span>
                <div>
                    <label className="switch">
                        <input type="checkbox" onClick={(e) => { this.changeLight(e) }} defaultChecked={info.isOn} />
                        <span className="slider round"></span>
                    </label>
                </div>
            </div>
        </div>;
    }
}