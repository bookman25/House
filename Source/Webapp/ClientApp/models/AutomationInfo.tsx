export interface AutomationInfo {
    id: string;
    title: string;
    status: string;
    icon: string;
    enabledClass: string;
    type: AutomationType;
    isOn: boolean;
}

export enum AutomationType {
    Lights,
    Climate
}